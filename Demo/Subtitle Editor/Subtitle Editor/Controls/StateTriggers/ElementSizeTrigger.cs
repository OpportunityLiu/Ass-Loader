using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SubtitleEditor.Controls.StateTriggers
{
    class ElementSizeTrigger : StateTriggerBase
    {
        public ElementSizeTrigger()
        {
        }

        public double MinElementWidth
        {
            get => (double)this.GetValue(MinElementWidthProperty);
            set => this.SetValue(MinElementWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for MinElementWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinElementWidthProperty =
            DependencyProperty.Register("MinElementWidth", typeof(double), typeof(ElementSizeTrigger), new PropertyMetadata((double)0, sizeChangedCallback));

        public double MinElementHeight
        {
            get => (double)this.GetValue(MinElementHeightProperty);
            set => this.SetValue(MinElementHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for MinElementHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinElementHeightProperty =
            DependencyProperty.Register("MinElementHeight", typeof(double), typeof(ElementSizeTrigger), new PropertyMetadata((double)0, sizeChangedCallback));

        private static void sizeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newVa = (double)e.NewValue;
            if(double.IsNaN(newVa) || newVa < 0)
            {
                if(e.Property == MinElementHeightProperty)
                    throw new ArgumentOutOfRangeException(nameof(MinElementHeight));
                if(e.Property == MinElementWidthProperty)
                    throw new ArgumentOutOfRangeException(nameof(MinElementWidth));
            }
            var target = ((ElementSizeTrigger)d).Target;
            if(target != null)
                targetCounter[target].NeedRefresh = true;
        }

        public FrameworkElement Target
        {
            get => (FrameworkElement)this.GetValue(TargetProperty);
            set => this.SetValue(TargetProperty, value);
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(ElementSizeTrigger), new PropertyMetadata(null, targetChangedCallback));

        private static Dictionary<FrameworkElement, triggerHelper> targetCounter = new Dictionary<FrameworkElement, triggerHelper>();

        private class triggerHelper : IDisposable
        {
            public triggerHelper(FrameworkElement target)
            {
                this.Target = target;
                target.SizeChanged += this.Target_SizeChanged;
            }

            private void Target_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                if(this.NeedRefresh)
                    this.reorderTriggers();
                for(int i = 0; i < this.triggers.Count; i++)
                {
                    if(!this.triggers[i].GetState(e))
                    {
                        for(int j = 0; j < this.triggers.Count; j++)
                        {
                            this.triggers[j].SetActive(j == i - 1);
                        }
                        return;
                    }
                }
                for(int j = 0; j < this.triggers.Count - 1; j++)
                {
                    this.triggers[j].SetActive(false);
                }
                this.triggers[this.triggers.Count - 1].SetActive(true);
            }

            private void reorderTriggers()
            {
                var newOrder = from trigger in this.triggers
                               orderby trigger.MinElementWidth, trigger.MinElementHeight
                               select trigger;
                this.triggers = newOrder.ToList();
                this.NeedRefresh = false;
            }

            public FrameworkElement Target
            {
                get;
                private set;
            }

            private List<ElementSizeTrigger> triggers = new List<ElementSizeTrigger>();

            // override object.Equals
            public override bool Equals(object obj)
            {
                var o = obj as triggerHelper;
                if(o == null)
                    return false;
                return this.Target == o.Target;
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                this.checkState();
                return this.Target.GetHashCode();
            }

            public void Dispose()
            {
                if(this.Disposed)
                    return;
                this.Target.SizeChanged -= this.Target_SizeChanged;
                this.Target = null;
                this.Disposed = true;
            }

            public void Add(ElementSizeTrigger trigger)
            {
                this.checkState();
                this.triggers.Add(trigger);
                this.NeedRefresh = true;
            }

            public void Remove(ElementSizeTrigger trigger)
            {
                this.checkState();
                this.triggers.Remove(trigger);
                if(this.triggers.Count == 0)
                    this.Dispose();
                else
                    this.NeedRefresh = true;
            }

            private void checkState()
            {
                if(this.Disposed)
                    throw new InvalidOperationException("helper disposed.");
            }

            public bool Disposed
            {
                get;
                private set;
            }

            public bool NeedRefresh
            {
                get;
                set;
            } = true;
        }

        private static void targetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (ElementSizeTrigger)d;
            if(e.OldValue != null)
                lock (targetCounter)
                {
                    var oldValue = (FrameworkElement)e.OldValue;
                    var oldCount = targetCounter[oldValue];
                    oldCount.Remove(trigger);
                    if(oldCount.Disposed)
                        targetCounter.Remove(oldValue);
                }
            if(e.NewValue!=null)
                lock (targetCounter)
                {
                    var newValue = (FrameworkElement)e.NewValue;
                    triggerHelper newCount;
                    if(targetCounter.TryGetValue(newValue, out newCount))
                    {
                        newCount.Add(trigger);
                    }
                    else
                    {
                        newCount = new triggerHelper(newValue);
                        newCount.Add(trigger);
                        targetCounter[newValue] = newCount;
                    }
                }
        }

        private bool GetState(SizeChangedEventArgs e)
        {
            return e.NewSize.Height >= this.MinElementHeight && e.NewSize.Width >= this.MinElementWidth;
        }
    }
}
