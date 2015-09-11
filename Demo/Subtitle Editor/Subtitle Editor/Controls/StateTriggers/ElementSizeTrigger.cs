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
            get
            {
                return (double)GetValue(MinElementWidthProperty);
            }
            set
            {
                SetValue(MinElementWidthProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MinElementWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinElementWidthProperty =
            DependencyProperty.Register("MinElementWidth", typeof(double), typeof(ElementSizeTrigger), new PropertyMetadata((double)0, sizeChangedCallback));

        public double MinElementHeight
        {
            get
            {
                return (double)GetValue(MinElementHeightProperty);
            }
            set
            {
                SetValue(MinElementHeightProperty, value);
            }
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
            get
            {
                return (FrameworkElement)GetValue(TargetProperty);
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(ElementSizeTrigger), new PropertyMetadata(null, targetChangedCallback));

        private static Dictionary<FrameworkElement, triggerHelper> targetCounter = new Dictionary<FrameworkElement, triggerHelper>();

        private class triggerHelper : IDisposable
        {
            public triggerHelper(FrameworkElement target)
            {
                Target = target;
                target.SizeChanged += Target_SizeChanged;
            }

            private void Target_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                if(NeedRefresh)
                    reorderTriggers();
                for(int i = 0; i < triggers.Count; i++)
                {
                    if(!triggers[i].GetState(e))
                    {
                        for(int j = 0; j < triggers.Count; j++)
                        {
                            triggers[j].SetActive(j == i - 1);
                        }
                        return;
                    }
                }
                for(int j = 0; j < triggers.Count - 1; j++)
                {
                    triggers[j].SetActive(false);
                }
                triggers[triggers.Count - 1].SetActive(true);
            }

            private void reorderTriggers()
            {
                var newOrder = from trigger in triggers
                               orderby trigger.MinElementWidth, trigger.MinElementHeight
                               select trigger;
                triggers = newOrder.ToList();
                NeedRefresh = false;
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
                checkState();
                return this.Target.GetHashCode();
            }

            public void Dispose()
            {
                if(Disposed)
                    return;
                Target.SizeChanged -= Target_SizeChanged;
                Target = null;
                Disposed = true;
            }

            public void Add(ElementSizeTrigger trigger)
            {
                checkState();
                triggers.Add(trigger);
                NeedRefresh = true;
            }

            public void Remove(ElementSizeTrigger trigger)
            {
                checkState();
                triggers.Remove(trigger);
                if(triggers.Count == 0)
                    Dispose();
                else
                    NeedRefresh = true;
            }

            private void checkState()
            {
                if(Disposed)
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
            return e.NewSize.Height >= MinElementHeight && e.NewSize.Width >= MinElementWidth;
        }
    }
}
