﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubtitleEditor.View
{
    interface IGoBack
    {
        void GoBack();

        bool CanGoBack
        {
            get;
        }

        event CanGoBackChangedEventHandler CanGoBackChanged;
    }

    delegate void CanGoBackChangedEventHandler(IGoBack sender, EventArgs e);
}
