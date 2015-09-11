using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SubtitleEditor.Controls.StateTriggers
{
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private static string current = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

        public string DeviceFamily
        {
            get
            {
                return (string)GetValue(DeviceFamilyProperty);
            }
            set
            {
                SetValue(DeviceFamilyProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for DeviceFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeviceFamilyProperty =
            DependencyProperty.Register("DeviceFamily", typeof(string), typeof(DeviceFamilyTrigger), new PropertyMetadata("Any", propertyChangedCallback));

        public bool ActiveIfMatch
        {
            get
            {
                return (bool)GetValue(ActiveIfMatchProperty);
            }
            set
            {
                SetValue(ActiveIfMatchProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ActiveIfMatch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveIfMatchProperty =
            DependencyProperty.Register("ActiveIfMatch", typeof(bool), typeof(DeviceFamilyTrigger), new PropertyMetadata(true, propertyChangedCallback));

        private static void propertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (DeviceFamilyTrigger)d;
            var value = trigger.DeviceFamily;
            bool match;
            if(string.Equals("Any", value, StringComparison.OrdinalIgnoreCase))
                match = true;
            else
                match = string.Equals(current, value, StringComparison.OrdinalIgnoreCase);
            trigger.SetActive(trigger.ActiveIfMatch ? match : !match);
        }
    }
}
