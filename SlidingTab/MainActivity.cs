using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SlidingTab
{
	[Activity (Label = "SlidingTab", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			FragmentTransaction transaction = FragmentManager.BeginTransaction ();
			SlidingTabsFragment fragment = new SlidingTabsFragment();
			transaction.Replace (Resource.Id.sample_content_fragment, fragment);
			transaction.Commit ();

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			//MenuInflater.Inflate(Resource.Menu.actionbar_main, menu);
			return base.OnCreateOptionsMenu (menu);
		}
	}
}


