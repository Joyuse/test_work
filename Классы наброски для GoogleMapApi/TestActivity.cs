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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Java.Net;
using Javax.Xml.Parsers;
using GooglePolyline = Android.Gms.Maps.Model;
using System.Text.RegularExpressions;


namespace GidForYou
{
	[Activity(Label = "TestActivity", Icon = "@mipmap/icon", Theme ="@style/Theme.AppCompat.Light.NoActionBar")]
	public class TestActivity : AppCompatActivity,IOnMapReadyCallback,ILocationListener
	{
		GoogleMap map;
		//Spinner spinner;
		LocationManager locationManager;
		String provider;
		//Marker MyPositionMarker;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.TestLatyout);
			MapFragment mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
			mapFragment.GetMapAsync(this);
			locationManager = (LocationManager)GetSystemService(Context.LocationService);
			provider = locationManager.GetBestProvider(new Criteria(), false);
              Location location = locationManager.GetLastKnownLocation(provider);
              if (location == null)
                  System.Diagnostics.Debug.WriteLine("No Location");
			
			var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);//Отображение наавигации
			bottomNavigation.NavigationItemSelected += (s, e) =>
			{
			switch (e.Item.ItemId)
			{//Дейсвтия по клику.
			case Resource.Id.action_map://Показать картку
			Toast.MakeText(this, "Action Map Clicked", ToastLength.Short).Show();
			break;
			case Resource.Id.action_monuments://Показать список
			Toast.MakeText(this, "Action Monuments Clicked", ToastLength.Short).Show();
			Intent MonumetList = new Intent(this, typeof(MainActivity));
			this.StartActivity(MonumetList);
			break;
			case Resource.Id.action_info://Показать "О нас"
			Toast.MakeText(this, "Action Tour Clicked", ToastLength.Short).Show();
			Intent About = new Intent(this, typeof(About_Actvity));
			this.StartActivity(About);
			break;
			}
			}; //Конец функции bottomNavigator
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			googleMap.UiSettings.ZoomControlsEnabled = true;
			googleMap.UiSettings.CompassEnabled = true;
			googleMap.MoveCamera(CameraUpdateFactory.ZoomIn());
		}

		protected override void OnResume()
		{
			base.OnResume();
			locationManager.RequestLocationUpdates(provider, 2000, 2, this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			locationManager.RemoveUpdates(this);
		}

		public string convertPoint2String(LatLng point)
		{
			string lat = point.Latitude.ToString().Replace(',', '.');
			string lng = point.Longitude.ToString().Replace(',', '.');
			return lat + "," + lng;
		}

		public async void OnLocationChanged(Location location)
		{
			map.Clear();
			//мои координаты
			double Mylat = location.Latitude;
			double Mylng = location.Longitude;
			LatLng startPoint = new LatLng(Mylat, Mylng);

			double MyLatitude = Intent.GetDoubleExtra("latitude", 1);
			double MyLongitude = Intent.GetDoubleExtra("longitude", 1);
			LatLng endPoint = new LatLng(MyLatitude, MyLongitude);

			MarkerOptions Point = new MarkerOptions().SetPosition(startPoint).SetTitle("Position").SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.human));
			map.AddMarker(Point);

			MarkerOptions makerOptions1 = new MarkerOptions().SetPosition(endPoint).SetTitle("ENDPOINT").SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.bank));
			map.AddMarker(makerOptions1);

			//Путь
			string start = convertPoint2String(startPoint);
			string end = convertPoint2String(endPoint);
			Console.WriteLine("START POINT NEW = " + start);
			Console.WriteLine("END POINT NEW = " + end);
			string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" + start + "&destination=" + end + "&key=KEY";
			Console.WriteLine("URLSTRING " + url);

			try
			{
				Console.WriteLine("1 = ");
				JsonValue MyWay = await GetDataFromReq(url);
				var r = MyWay.ToString();
				RootObject jsonline = JsonConvert.DeserializeObject<RootObject>(r);
				Console.WriteLine("2 = ");
				Console.WriteLine("!!!!!!!!!!!" + r);
				List<LatLng> resultWay = new List<LatLng>();

				jsonline.routes.Where(t => t != null).ToList().ForEach(
					tr => tr.legs.Where(lg => lg != null).ToList().ForEach(
						lgs => lgs.steps.Where(st => st != null).ToList().ForEach(
							legs => resultWay= resultWay.Union(DecodePolyline(legs.polyline.points)).ToList()
						)
					)
				);
				resultWay.ForEach(t => Console.WriteLine("p:" + t));
				PolylineOptions opt = new PolylineOptions();
				opt.Add(resultWay.ToArray());
				map.AddPolyline(opt);
			}
			catch (Exception e) { }

			//Передвижение
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
			builder.Target(new LatLng(Mylat, Mylng));
			builder.Target(startPoint);
			builder.Zoom(18);
			builder.Bearing(155);
			CameraPosition cameraPosition = builder.Build();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
			map.MoveCamera(cameraUpdate);
		}

		public static List<LatLng> DecodePolyline(string encodedPoints)
		{
			if (string.IsNullOrEmpty(encodedPoints))
				throw new ArgumentNullException("encodedPoints");
			char[] polylineChars = encodedPoints.ToCharArray();
			int index = 0;
			int currentLat = 0;
			int currentLng = 0;
			int next5bits;
			int sum;
			int shifter;
			List<LatLng> polylinesPosition = new List<LatLng>();
			while (index < polylineChars.Length)
			{
				// calculate next latitude
				sum = 0;
				shifter = 0;
				do
				{
					next5bits = (int)polylineChars[index++] - 63;
					sum |= (next5bits & 31) << shifter;
					shifter += 5;
				}

				while (next5bits >= 32 && index < polylineChars.Length);
				if (index >= polylineChars.Length)
					break;
				currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
				//calculate next longitude
				sum = 0;
				shifter = 0;
				do
				{
					next5bits = (int)polylineChars[index++] - 63;
					sum |= (next5bits & 31) << shifter;
					shifter += 5;
				} while (next5bits >= 32 && index < polylineChars.Length);
				if (index >= polylineChars.Length && next5bits >= 32)
					break;
				currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

				polylinesPosition.Add(new LatLng(Convert.ToDouble(currentLat) / 1E5, Convert.ToDouble(currentLng) / 1E5));
			}
			return (polylinesPosition);  
		}

		//Запрос документа пути Google
		public async Task<JsonValue> GetDataFromReq(string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";
			using (WebResponse response = await request.GetResponseAsync())
			{
				using (Stream stream = response.GetResponseStream())
				{
					JsonValue WayStream = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("value: {0}", WayStream.ToString());
					return WayStream;
				}
			}
		}

		public void OnProviderDisabled(string provider)
		{
			// throw new NotImplementedException();
		}

		public void OnProviderEnabled(string provider)
		{
			// throw new NotImplementedException();
		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{
			// throw new NotImplementedException();		}
	}
}
