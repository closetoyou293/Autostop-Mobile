﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autostop.Client.Abstraction.Adapters;
using Autostop.Client.Abstraction.Factories;
using Autostop.Client.Abstraction.Services;
using Autostop.Client.Abstraction.ViewModels;
using Autostop.Client.Android.Views;
using Autostop.Client.Core.IoC;

namespace Autostop.Client.Android.Services
{
	public class NavigationService : INavigationService
	{
	    private readonly IViewAdapter<Fragment> _viewAdapter;
	    private readonly IViewFactory _viewFactory;
	    private readonly Activity _rootActivity;

        public NavigationService(
            IViewAdapter<Fragment> viewAdapter, 
            IViewFactory viewFactory)
        {
            _viewAdapter = viewAdapter;
            _viewFactory = viewFactory;
            _rootActivity = RootActivity.Instance;
        }
		public void NavigateTo(Type viewModelType)
		{
		    var fragment = GetFragment(viewModelType);
            ReplaceContent(fragment);
		}

		public void NavigateTo(Type viewModelType, Action<object> configure)
		{
		    var fragment = GetFragment(viewModelType);
		    configure(fragment);
            ReplaceContent(fragment);
		}

        public void NavigateTo<TViewModel>()
        {
            var viewModel = Locator.Resolve<TViewModel>();
            var fragment = GetFragment(viewModel);
            ReplaceContent(fragment);
        }

		public void NavigateTo<TViewModel>(TViewModel viewModel)
		{
		    var fragment = GetFragment(viewModel);
		    ReplaceContent(fragment);
        }

		public void NavigateToModal<TViewModel>()
		{
			throw new NotImplementedException();
		}

		public void NavigateTo<TViewModel>(Action<object, TViewModel> configure)
		{
		    var viewModel = Locator.Resolve<TViewModel>();
		    var fragment = GetFragment(viewModel);
		    configure(fragment, viewModel);
		    ReplaceContent(fragment);
        }

		public void NavigateToSearchView<TViewModel>(Action<TViewModel> callBack) where TViewModel : ISearchableViewModel
		{
			throw new NotImplementedException();
		}

		public void NavigateToSearchView<TViewModel>(TViewModel viewModel) where TViewModel : ISearchableViewModel
		{
			throw new NotImplementedException();
		}

		public void GoBack()
		{
            _rootActivity.FragmentManager.PopBackStack();
		}

		public void NavigaeToRoot()
		{
			throw new NotImplementedException();
		}

	    private Fragment GetFragment<TViewModel>(TViewModel viewModel)
	    {
	        var view = _viewFactory.CreateView(viewModel);
	        var fragment = _viewAdapter.GetView(view);
	        return fragment;
	    }

        private Fragment GetFragment(Type viewModelType)
	    {
	        var viewModel = Locator.Resolve(viewModelType);
	        var createView = typeof(IViewFactory).GetMethod(nameof(IViewFactory.CreateView))
	            ?.MakeGenericMethod(viewModelType);
	        var screenFor = createView?.Invoke(this, new[] { viewModel });
	        var getView = typeof(IViewAdapter<Fragment>)
	            .GetMethod(nameof(IViewAdapter<Fragment>.GetView))
	            ?.MakeGenericMethod(viewModelType);

	        var fragment = getView?.Invoke(this, new[] { screenFor }) as Fragment;
	        return fragment;
	    }

	    private void ReplaceContent(Fragment fragment)
	    {
	        var fragmentManager = _rootActivity.FragmentManager;

	        if (fragmentManager.BackStackEntryCount > 0)
	        {
	            fragmentManager
	                .BeginTransaction()
	                .Replace(Resource.Id.container, fragment)
	                .AddToBackStack(null)
	                .Commit();
	        }
	        else
	        {
	            fragmentManager
	                .BeginTransaction()
	                .Replace(Resource.Id.container, fragment)
	                .Commit();
	        }
	    }
    }
}