using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace SlidingTab
{
	public class SlidingTabStrip : LinearLayout
	{
		private const int DEFAULT_BOT_BORDER_THICKNESS_DIPS = 2;
		private const byte DEFAULT_BOT_BORDER_COLOR_ALPHA = 0X26;
		private const int SELECTED_INDICATOR_THICKNESS_DIPS = 8;
		private int[] INDICATOR_COLORS = { 0x19A319, 0x148dfa };
		private int[] DIVIDER_COLORS = {0xC5C5C5};

		private const int DEFAULT_DIV_THICKNESS_DIPS = 1;
		private const float DEFAULT_DIV_HEIGHT = 0.5f;

		//bot border
		private int mBotBorderThickness;
		private Paint mBotBorderPaint;
		private int mDefaultBotBorderColor;

		//indicator
		private int mSelectedIndicatorThickness;
		private Paint mSelectedIndicatorPaint;

		//divider
		private Paint mDividerpaint;
		private float mDividerHeight;

		//selected pos and offset
		private int mSelectedPos;
		private float mSelectionOffset;

		//tab colorizer
		private SlidingTabScrollView.TabColorizer mCustomTabColorizer;
		private SimpleTabColorizer mDefaultTabColorizer;

		//constructors
		public SlidingTabStrip(Context context) : this(context, null)
		{
		}


		public SlidingTabStrip (Context context, IAttributeSet attrs) : base(context, attrs)
		{
			SetWillNotDraw (false);

			float density = Resources.DisplayMetrics.Density;

			TypedValue outValue = new TypedValue ();
			context.Theme.ResolveAttribute (Android.Resource.Attribute.ColorForeground, outValue, true);
			int themeForeGround = outValue.Data;
			mDefaultBotBorderColor = SetColorAlpha (themeForeGround, DEFAULT_BOT_BORDER_COLOR_ALPHA);

			mDefaultTabColorizer = new SimpleTabColorizer ();
			mDefaultTabColorizer.IndicatorColors = INDICATOR_COLORS;
			mDefaultTabColorizer.DividerColors = DIVIDER_COLORS;

			mBotBorderThickness = (int)(DEFAULT_BOT_BORDER_THICKNESS_DIPS * density);
			mBotBorderPaint = new Paint ();
			mBotBorderPaint.Color = GetColorFromInteger (0x148dfa);

			mSelectedIndicatorThickness = (int)(SELECTED_INDICATOR_THICKNESS_DIPS * density);
			mSelectedIndicatorPaint = new Paint ();


			mDividerHeight = DEFAULT_DIV_HEIGHT;
			mDividerpaint = new Paint ();
			mDividerpaint.StrokeWidth = (int)(DEFAULT_DIV_THICKNESS_DIPS * density);

		}

		public SlidingTabScrollView.TabColorizer CustomTabColorizer
		{
			set {
				mCustomTabColorizer = value;
				this.Invalidate();
			}
		}

		public int[] SelectedIndicatorColors
		{
			set { 
				mCustomTabColorizer = null;
				mDefaultTabColorizer.IndicatorColors = value;
				this.Invalidate ();
			}
		}

		public int [] DividerColors
		{
			set {
				mDefaultTabColorizer = null;
				mDefaultTabColorizer.DividerColors = value;
				this.Invalidate ();
			}
		}

		private Color GetColorFromInteger(int color)
		{
			return Color.Rgb (Color.GetRedComponent (color), Color.GetGreenComponent (color), Color.GetBlueComponent (color));
		}



			private	int SetColorAlpha (int color, byte alpha)
			{
			return Color.Argb (alpha, Color.GetRedComponent (color), Color.GetGreenComponent (color), Color.GetBlueComponent (color));
			}

		public void OnViewPagerPageChanged(int pos, float posOffset)
		{
			mSelectedPos = pos;
			mSelectionOffset = posOffset;
			this.Invalidate();
		}



		protected override void OnDraw(Canvas canvas)
		{
			int height = Height;
			int tabCount = ChildCount;
			int dividerHeightPx = (int)(Math.Min (Math.Max (0f, mDividerHeight), 1f) * height);
			SlidingTabScrollView.TabColorizer tabColorizer = mCustomTabColorizer != null ? mCustomTabColorizer : mDefaultTabColorizer;
			//thick cololred underline below selection
			if (tabCount > 0) {
				View selectedTitle = GetChildAt (mSelectedPos);
				int left = selectedTitle.Left;
				int right = selectedTitle.Right;
				int color = tabColorizer.GetIndicatorColor (mSelectedPos);

				if (mSelectionOffset > 0f && mSelectedPos < (tabCount - 1)) {
					int nextColor = tabColorizer.GetIndicatorColor(mSelectedPos + 1);
					if(color != nextColor){
						color = blendColor(nextColor, color, mSelectionOffset);
					}
					//left and right coord of next child. where it needs to end up at
					View nextTitle = GetChildAt (mSelectedPos + 1);
					left = (int)(mSelectionOffset * nextTitle.Left + (1.0f - mSelectionOffset) * left);
					right = (int)(mSelectionOffset * nextTitle.Right + (1.0f - mSelectionOffset) * right);
				}
				mSelectedIndicatorPaint.Color = GetColorFromInteger (color);

				canvas.DrawRect (left, height - mSelectedIndicatorThickness, right, height, mSelectedIndicatorPaint);

				//create vertical dividers between tabs
				int separatorTop = (height - dividerHeightPx) / 2;
				for (int i = 0; i < ChildCount; i++) {
					View child = GetChildAt (i);
					mDividerpaint.Color = GetColorFromInteger (tabColorizer.GetDividerColor (i));
					canvas.DrawLine (child.Right, separatorTop, child.Right, separatorTop + dividerHeightPx, mDividerpaint);
				}
				canvas.DrawRect (0, height - mBotBorderThickness, Width, height, mBotBorderPaint);
			}
		}
		int blendColor (int color1, int color2, float ratio)
		{
			float inverseRatio = 1f - ratio;
			float r = (Color.GetRedComponent (color1) * ratio) + (Color.GetRedComponent (color2) * inverseRatio);
			float g = (Color.GetGreenComponent (color1) * ratio) + (Color.GetGreenComponent (color2) * inverseRatio);
			float b = (Color.GetBlueComponent (color1) * ratio) + (Color.GetBlueComponent (color2) * inverseRatio);

			return Color.Rgb ((int)r, (int)g, (int)b);
		}

		private class SimpleTabColorizer : SlidingTabScrollView.TabColorizer
		{

			private int[] mIndicatorColors;
			private int[] mDividerColors;

			public int GetIndicatorColor(int pos)
			{
				return mIndicatorColors[pos % mIndicatorColors.Length];
			}

			//divider colors
			public int GetDividerColor(int pos)
			{
				return mDividerColors [pos % mDividerColors.Length];
			}

			public int[] IndicatorColors
			{
				set { mIndicatorColors = value; }
			}

			public int[] DividerColors {
				set { mDividerColors = value; }
			}
		}
		}
	}


