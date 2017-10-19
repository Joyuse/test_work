package com.example.user.my_test_project;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.support.annotation.LayoutRes;
import android.support.annotation.NonNull;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;

/**
 * Created by User on 19.10.2017.
 */

public class CustomAdaper extends ArrayAdapter<String> {

    Context context;
    String [] text;
    String [] bats;
    LayoutInflater inflater;
    Button favoriteButton,editButton;
    Dialog dialog;

    public CustomAdaper(Context context, String[] text, String[] bats) {
        super(context, R.layout.adapter_layout,text);
        this.context = context;
        this.text = text;
        this.bats = bats;
    }

    public class ViewWe {
        TextView text_view;
        ImageView bats_view;
        Button favoriteButton,editButton;
    }

    @Override
    public View getView (final int position, View contentView, final ViewGroup parent){
      if (contentView == null){
          inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
          contentView = inflater.inflate(R.layout.adapter_layout,null);
      }

      final ViewWe viewWe = new ViewWe();
        viewWe.text_view = (TextView) contentView.findViewById(R.id.short_text);
        viewWe.bats_view = (ImageView) contentView.findViewById(R.id.image_int);
        viewWe.favoriteButton = (Button) contentView.findViewById(R.id.add_favorite);
        viewWe.editButton = (Button) contentView.findViewById(R.id.edit_text);

        Glide.with(context)
                .load(bats[position])
                .into(viewWe.bats_view);
       // viewWe.text_view.setText(text[position]);
        viewWe.editButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Toast.makeText(context, "BUTTON WAS CLICKERD " +position, Toast.LENGTH_SHORT).show();

                dialog = new Dialog(context);
                dialog.setContentView(R.layout.context_menu_context);

                final EditText edite_text = (EditText) dialog.findViewById(R.id.edit_text_context);
                Button buttonChange = (Button) dialog.findViewById(R.id.change);
                Button buttonNonChange = (Button) dialog.findViewById(R.id.no_change);

                edite_text.setEnabled(true);
                buttonChange.setEnabled(true);
                buttonNonChange.setEnabled(true);
                dialog.show();

                buttonChange.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        viewWe.text_view.setText(edite_text.getText());
                        Toast.makeText(context, "TEXT EDITED ", Toast.LENGTH_SHORT).show();
                        dialog.cancel();
                    }
                });

                buttonNonChange.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        Toast.makeText(context, "TEXT NO EDITED ", Toast.LENGTH_SHORT).show();
                        dialog.cancel();
                    }
                });
            }
        });
      return  contentView;
    }
}
