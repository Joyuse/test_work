package com.example.user.my_test_project;

import android.content.Context;
import android.support.annotation.LayoutRes;
import android.support.annotation.NonNull;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.bumptech.glide.Glide;

/**
 * Created by User on 19.10.2017.
 */

public class CustomAdaper extends ArrayAdapter<String> {

    String[] internetUrl = {
            "http://i.imgur.com/aBpXLHy.jpg",
            "https://i.ytimg.com/vi/J1usv2Hn-pU/maxresdefault.jpg",
            "https://www.belanta.vet/vet-blog/wp-content/uploads/2016/08/kotenok-myaukaet-v-tualete-2.jpg",
    };

    Context context;
    String [] text;
    int [] bats;
    LayoutInflater inflater;

    public CustomAdaper(Context context, String[] text, int[] bats) {
        super(context, R.layout.adapter_layout,text);

        this.context = context;
        this.text = text;
        this.bats = bats;
    }

    public class ViewWe {
        TextView text_view;
        ImageView bats_view;
    }

    @Override
    public View getView (int position, View contentView, ViewGroup parent){
      if (contentView == null){
          inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
          contentView = inflater.inflate(R.layout.adapter_layout,null);
      }

      ViewWe viewWe = new ViewWe();
        viewWe.text_view = (TextView) contentView.findViewById(R.id.short_text);
        viewWe.bats_view = (ImageView) contentView.findViewById(R.id.image_int);

        Glide.with(context)
                .load("https://pp.userapi.com/c840334/v840334425/16bc1/vPLMogBbbcA.jpg")
                .into(viewWe.bats_view); 

        viewWe.text_view.setText(text[position]);
        viewWe.bats_view.setImageResource(bats[position]);

      return  contentView;
    }
}
