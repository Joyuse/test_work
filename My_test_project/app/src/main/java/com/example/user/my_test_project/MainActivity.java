package com.example.user.my_test_project;

import android.app.ListActivity;
import android.content.Context;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.widget.CursorAdapter;
import android.widget.ImageView;

import com.bumptech.glide.Glide;



public class MainActivity extends ListActivity {

//    String[] internetUrl = {
//            "http://i.imgur.com/aBpXLHy.jpg",
//            "https://i.ytimg.com/vi/J1usv2Hn-pU/maxresdefault.jpg",
//            "https://www.belanta.vet/vet-blog/wp-content/uploads/2016/08/kotenok-myaukaet-v-tualete-2.jpg",
//    };

    String[] text = {
            "1","2","3"
    };

    int[] img = {
      R.drawable.bat1, R.drawable.bat2, R.drawable.bat3
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        CustomAdaper adapter = new CustomAdaper(this,text,img);
        setListAdapter(adapter);
    }
}
