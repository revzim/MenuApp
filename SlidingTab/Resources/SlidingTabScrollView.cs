using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Content.Res;
using Android.Widget;
using Android.Graphics;
using Android.Util;
using Android.Support.V4.View;


namespace SlidingTab
{
	public class SlidingTabScrollView : HorizontalScrollView
	{

		private const int TITLE_OFFSET_DIPS = 24;
		private const int TAB_VIEW_PADDING = 16;
		private const int TAB_VIEW_TEXT_SIZE_SP = 12;

		private int mTitleOffset;

		private int mTabViewLayoutId;
		private int mTabViewTextViewId;

		private ViewPager mViewPager;
		private ViewPager.IOnPageChangeListener mViewPagerPageChangeListener;

		private static SlidingTabStrip mTabStrip;

		private int mScrollState;


		public interface TabColorizer
		{
			int GetIndicatorColor(int pos);
			int GetDividerColor(int pos);
		}

		public SlidingTabScrollView(Context context) : this(context, null) {
		}
		public SlidingTabScrollView(Context context, IAttributeSet attrs) : this(context, attrs, 0) {
		}
		public SlidingTabScrollView(Context context, IAttributeSet attrs, int defaultStyle) : base(context, attrs, defaultStyle)
		{
			//disable scroll bar

			HorizontalScrollBarEnabled = false;

			//make sure tab strips fill view
			FillViewport = true;
			this.SetBackgroundColor(Android.Graphics.Color.Rgb(0xE5, 0xE5, 0xE5)); //gray

			mTitleOffset = (int)(TITLE_OFFSET_DIPS * Resources.DisplayMetrics.Density);

			mTabStrip = new SlidingTabStrip (context);
			this.AddView (mTabStrip, LayoutParams.MatchParent, LayoutParams.MatchParent);


		}
		public TabColorizer CustomTabColorizer
		{
			set { mTabStrip.CustomTabColorizer = value; }
		}

		public int [] SelectedIndicatorColor
		{
			set { mTabStrip.SelectedIndicatorColors = value; }
		}

		public int [] DividerColors
		{
			set { mTabStrip.DividerColors = value; }
		}

		public ViewPager.IOnPageChangeListener OnPageListener
		{
			set { mViewPagerPageChangeListener = value; }
		}

		public ViewPager ViewPager
		{
			set {
				mTabStrip.RemoveAllViews ();

				mViewPager = value;
				if (value != null) {
					value.PageSelected += Value_PageSelected;
					value.PageScrollStateChanged += Value_PageScrollStateChanged;
					value.PageScrolled += Value_PageScrolled;
					PopulateTabStrip ();
				}
			}
		}


		void Value_PageSelected (object sender, ViewPager.PageSelectedEventArgs e)
		{
			if (mScrollState == ViewPager.ScrollStateIdle) {
				mTabStrip.OnViewPagerPageChanged (e.Position, 0f);
				ScrollToTab (e.Position, 0);
			}
			if (mViewPagerPageChangeListener != null) {
				mViewPagerPageChangeListener.OnPageSelected (e.Position);
			}
		}

		void Value_PageScrollStateChanged (object sender, ViewPager.PageScrollStateChangedEventArgs e)
		{
			mScrollState = e.State;

			if (mViewPagerPageChangeListener != null) {
				mViewPagerPageChangeListener.OnPageScrollStateChanged (e.State);
			}
		}



		void Value_PageScrolled (object sender, ViewPager.PageScrolledEventArgs e)
		{
			int tabCount = mTabStrip.ChildCount;

			if ((tabCount == 0) || (e.Position < 0) || (e.Position >= tabCount)) {
				//if any of these conditions apply, just return;
				return;
			}

			mTabStrip.OnViewPagerPageChanged (e.Position, e.PositionOffset);

			View selectedTitle = mTabStrip.GetChildAt (e.Position);

			int extraOffset = (selectedTitle != null) ? (int)(e.Position * selectedTitle.Width) : 0;

			ScrollToTab(e.Position, extraOffset);

			if(mViewPagerPageChangeListener != null)
			{
				mViewPagerPageChangeListener.OnPageScrolled(e.Position, e.PositionOffset, e.PositionOffsetPixels);
			}

		

		}


		private void PopulateTabStrip()
		{
			PagerAdapter adapter = mViewPager.Adapter;

			for (int i = 0; i < adapter.Count; i++) {
				TextView tabView = CreateDefaultTabView (Context);
				//downcast to subclass approp header title
				tabView.Text = ((SlidingTabsFragment.SamplePagerAdapter)adapter).GetHeaderTitle (i);
				tabView.SetTextColor (Android.Graphics.Color.Black);
				//sets corresponding page to tab
				tabView.Tag = i;
				tabView.Click += TabView_Click;
				mTabStrip.AddView (tabView);
			}
		}

		void TabView_Click (object sender, EventArgs e)
		{
			TextView clickTab = (TextView)sender;
			int pageToScrollTo = (int)clickTab.Tag;
			//calls statechange lets scroll
			mViewPager.CurrentItem = pageToScrollTo;
		}


		TextView CreateDefaultTabView (Context context)
		{
			TextView textView = new TextView (context);
			textView.Gravity = GravityFlags.Center;
			textView.SetTextSize (ComplexUnitType.Sp, TAB_VIEW_TEXT_SIZE_SP);
			textView.Typeface = Android.Graphics.Typeface.DefaultBold;

			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb) {
				TypedValue outValue = new TypedValue ();
				Context.Theme.ResolveAttribute (Android.Resource.Attribute.SelectableItemBackground, outValue, false);
				textView.SetBackgroundResource (outValue.ResourceId);
			}

			if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich) {
				textView.SetAllCaps (true);
			}

			int padding = (int)(TAB_VIEW_PADDING * Resources.DisplayMetrics.Density);
			textView.SetPadding (padding, padding, padding, padding);

			return textView;

		}


		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			if (mViewPager != null) {
				ScrollToTab (mViewPager.CurrentItem, 0);
			}
		}


		void ScrollToTab (int tabIndex, int extraOffset)
		{
			int tabCount = mTabStrip.ChildCount;

			if (tabCount == 0 || tabIndex < 0 || tabIndex >= tabCount) {
				//no need to go further and don't scroll
				return;
			}

			View selectedChild = mTabStrip.GetChildAt (tabIndex);
			if (selectedChild != null) {
				int scrollAmountX = selectedChild.Left + extraOffset;

				if (tabIndex > 0 || extraOffset > 0) {
					scrollAmountX -= mTitleOffset;
				}

				this.ScrollTo (scrollAmountX, 0);
			}
		}

	}

}

