package com.example.user.my_test_project;

import android.app.Dialog;
import android.app.ListActivity;
import android.content.Context;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.CursorAdapter;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.Toast;

import com.bumptech.glide.Glide;


public class MainActivity extends ListActivity {

    String[] text = {
            "1","2","3"
    };

    String[] img = {
            "https://i.pinimg.com/736x/e5/a0/69/e5a06942fa42823c88be5f3a834e063d--fantastic-art-bat-family.jpg",
            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTAUHN_CbDP7sKlXbpD0A6uEO14OVNqCqxenGWJjwDIC3Otg9_S",
            "https://i.pinimg.com/736x/47/36/62/47366279cc3c500c9301b80fab304715--chibi-batman-batman-art.jpg"
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        CustomAdaper adapter = new CustomAdaper(this,text,img);
        setListAdapter(adapter);
    }
}
