using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace GidForYou
{
	[Activity(Label = "GidForYou", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
	public class MainActivity : AppCompatActivity
	{
		ListView lv2;

		public class items
		{
			[JsonProperty("response")]
			public List<categories> item { get; set; }
		}

		public class categories
		{
			[JsonProperty("id")]
			public int id { get; set; }
			[JsonProperty("name")]
			public string name { get; set; }
			[JsonProperty("shortDesc")]
			public string shortDesc { get; set; }
			[JsonProperty("longDesc")]
			public string longDesc { get; set; }
			[JsonProperty("longitude")]
			public double? longitude { get; set; }
			[JsonProperty("latitude")]
			public double? latitude { get; set; }
			[JsonProperty("likes")]
			public int likes { get; set; }
		}

		public class HomeScreenAdapter : BaseAdapter<categories>
		{
			List<categories> categories = new List<categories>();
			Activity context;
			public HomeScreenAdapter(Activity context, List<categories> categories)	
					: base()
			{
				this.context = context;
				this.categories = categories;
			}
			public override long GetItemId(int position)
			{
				return position;
			}
			public override categories this[int position]
			{
				get { return categories[position]; }
			}
			public override int Count
			{
				get { return categories.Count; }
			}
			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				var item = categories[position];
				View view = convertView;
				if (view == null) // no view to re-use, create new
					view = context.LayoutInflater.Inflate(Resource.Layout.MonumentView, null);
					//view.FindViewById<TextView>(Resource.Id.Text1).Text = item.id.ToString();
					view.FindViewById<TextView>(Resource.Id.Text2).Text = item.name;
					view.FindViewById<TextView>(Resource.Id.Text3).Text = item.shortDesc;
					//view.FindViewById<TextView>(Resource.Id.Text4).Text = item.longitude.ToString();
					//view.FindViewById<TextView>(Resource.Id.Text5).Text = item.latitude.ToString();
					//view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
					return view;
			}
		}

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			// Create your application here
			string url = "http://opendata.dev.zennex.ru/monument";
			JsonValue json = await GetCategories(url);
			var r = json.ToString();
			Console.WriteLine(r);
			var ready = JsonConvert.DeserializeObject<List<categories>>(r);
			Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			Console.WriteLine("item {0}, Всего {1}", ready.Count, ready);
			lv2 = FindViewById<ListView>(Resource.Id.List1);
			lv2.Adapter = new HomeScreenAdapter(this, ready);
			lv2.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				Console.WriteLine("position = " + e.Position + " latitude = " + ready[e.Position].latitude + "longitude = "+ ready[e.Position].longitude);
			};

			var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);//Отображение наавигации
			bottomNavigation.NavigationItemSelected += (s, e) =>
			{
			switch (e.Item.ItemId)
			{//Дейсвтия по клику.
			case Resource.Id.action_map://Показать картку
			Toast.MakeText(this, "Action Map Clicked", ToastLength.Short).Show();
			//Intent MapActivity = new Intent(this, typeof(MainActivity));
			//this.StartActivity(MapActivity);
			break;
			case Resource.Id.action_monuments://Показать список
			Toast.MakeText(this, "Action Monuments Clicked", ToastLength.Short).Show();
			break;
			case Resource.Id.action_info://Показать "О нас"
			Toast.MakeText(this, "Action Tour Clicked", ToastLength.Short).Show();
			break;
			}
			}; //Конец функции bottomNavigator

		}
		private async Task<JsonValue> GetCategories(string url)
		{
			// Create an HTTP web request using the URL:
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";
			// Send the request to the server and wait for the response:
			using (WebResponse response = await request.GetResponseAsync())
			{
				// Get a stream representation of the HTTP web response:
				using (Stream stream = response.GetResponseStream())
				{
				// Use this stream to build a JSON document object:
					JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					// Return the JSON doc ument:
					return jsonDoc;
				}
			}
		}
	}
}

