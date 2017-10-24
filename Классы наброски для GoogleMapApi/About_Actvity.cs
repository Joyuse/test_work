
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace GidForYou
{
	[Activity(Label = "About_Actvity", Theme ="@style/Theme.AppCompat.Light.NoActionBar")]
	public class About_Actvity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.About_Layout);
			var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);//Отображение наавигации
			bottomNavigation.NavigationItemSelected += (s, e) =>
			{
			switch (e.Item.ItemId)
			{
			case Resource.Id.action_map://Показать картку
			Toast.MakeText(this, "Action Map Clicked", ToastLength.Short).Show();
			Intent Map = new Intent(this, typeof(TestActivity));
			this.StartActivity(Map);
			break;
			case Resource.Id.action_monuments://Показать список
			Toast.MakeText(this, "Action Monuments Clicked", ToastLength.Short).Show();
			Intent MonumetList = new Intent(this, typeof(MainActivity));
			this.StartActivity(MonumetList);
			break;
			case Resource.Id.action_info://Показать "О нас"
			Toast.MakeText(this, "Action Tour Clicked", ToastLength.Short).Show();
			break;
			}
			}; 
		}
	}
}
