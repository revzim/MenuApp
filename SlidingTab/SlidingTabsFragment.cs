
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View.Accessibility;
using Android.Support.V4.View;

using SlidingTab;

namespace SlidingTab 
{
	
	public class SlidingTabsFragment : Fragment
	{
		
		private SlidingTabScrollView mSlidingTabScrollView;
		private ViewPager mViewPager;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate (Resource.Layout.fragment_sample, container, false);
		}
		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			mSlidingTabScrollView = view.FindViewById<SlidingTabScrollView> (Resource.Id.sliding_tabs);
			mViewPager = view.FindViewById<ViewPager> (Resource.Id.viewpager);
			mViewPager.Adapter = new SamplePagerAdapter ();
			mSlidingTabScrollView.ViewPager = mViewPager;

		}
		public class SamplePagerAdapter : PagerAdapter
		{

			List<string> items = new List<string>();


			public SamplePagerAdapter() : base()
			{
				items.Add("Breakfast");
				items.Add("Lunch");
				items.Add("Dinner");
				items.Add("Dessert");
				items.Add("Cocktails");
				items.Add("About");
			}

			public override int Count {
				get {
					return items.Count;

				}

			}
			public override bool IsViewFromObject(View view, Java.Lang.Object obj)
			{
				return view == obj;
			}
			public override Java.Lang.Object InstantiateItem (ViewGroup container, int position)
			{
				View view = LayoutInflater.From (container.Context).Inflate (Resource.Layout.pager_item, container, false);

				container.AddView (view);
				TextView txtTitle = view.FindViewById<TextView> (Resource.Id.item_title);
				int pos = position + 1;
				txtTitle.Text = pos.ToString ();
				return view;
			}

			public string GetHeaderTitle(int pos)
			{
				//gets title for tabs position
				return items[pos];
			}

			public override void DestroyItem (ViewGroup container, int position, Java.Lang.Object obj)
			{
				//casts and removes the view
				container.RemoveView ((View)obj);
			}

		}
	}
}

