﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrumpSoftware.Mvvm;

namespace TrumpSoftware.Wpf
{
    public class WindowNavigationHost : INavigationHost
    {
        private readonly NavigationWindow _navigationWindow;
        private readonly IDictionary<Type, Page> _pages = new Dictionary<Type, Page>();
        private readonly IList<PageViewModel> _history = new List<PageViewModel>();

        public WindowNavigationHost(NavigationWindow navigationWindow)
        {
            if (navigationWindow == null)
                throw new ArgumentNullException("navigationWindow");
            _navigationWindow = navigationWindow;
        }

        public void Register<TPageVM>(Page page)
            where TPageVM : PageViewModel
        {
            if (_pages.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pages.Add(typeof (TPageVM), page);
        }

        public void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel
        {
            var navigatingPageVMType = pageVM.GetType();
            if (!_pages.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            _history.Add(pageVM);
            var page = _pages[navigatingPageVMType];
            page.DataContext = pageVM;
            _navigationWindow.Navigate(page, pageVM);
        }

        public bool CanGoBack
        {
            get { return _history.Any(); }
        }

        public void GoBack()
        {
            var lastPageVM = _history.LastOrDefault();
            if (lastPageVM != null)
                Navigate(lastPageVM);
        }
    }
}