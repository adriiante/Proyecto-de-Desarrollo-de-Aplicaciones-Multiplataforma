package com.inforace;

import android.os.AsyncTask;
import android.os.Bundle;
import androidx.appcompat.app.AppCompatActivity;
import android.util.Log;
import android.webkit.WebView;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.StringReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.TimeZone;

public class F1Activity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.f1_activity);

        new FetchDriverStandings().execute();
        new FetchConstructorStandings().execute();
        new FetchSchedule().execute();
    }

    private class FetchDriverStandings extends AsyncTask<Void, Void, String> {

        @Override
        protected String doInBackground(Void... voids) {
            String apiUrl = "https://ergast.com/api/f1/current/driverStandings";

            try {
                URL url = new URL(apiUrl);
                HttpURLConnection connection = (HttpURLConnection) url.openConnection();
                connection.setRequestMethod("GET");

                InputStream inputStream = connection.getInputStream();
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null) {
                    stringBuilder.append(line).append("\n");
                }
                bufferedReader.close();
                return stringBuilder.toString();
            } catch (IOException e) {
                Log.e("F1Activity", "Error al obtener datos de la API", e);
                return null;
            }
        }

        @Override
        protected void onPostExecute(String result) {
            if (result != null) {
                Log.d("F1Activity", "Respuesta de la API: " + result);
                try {
                    String parsedData = parseDriverStandingsXML(result);
                    WebView apiResponseWebView = findViewById(R.id.pilotsApiResponseWebView);
                    apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                    apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
                } catch (Exception e) {
                    Log.e("F1Activity", "Error al parsear los datos del piloto", e);
                }
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseDriverStandingsXML(String xml) throws XmlPullParserException, IOException {
            XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
            XmlPullParser parser = factory.newPullParser();
            parser.setInput(new StringReader(xml));

            List<String[]> driverStandings = new ArrayList<>();
            int eventType = parser.getEventType();
            while (eventType != XmlPullParser.END_DOCUMENT) {
                if (eventType == XmlPullParser.START_TAG && parser.getName().equals("DriverStanding")) {
                    String position = parser.getAttributeValue(null, "position");
                    String points = parser.getAttributeValue(null, "points");
                    String wins = parser.getAttributeValue(null, "wins");
                    String givenName = "";
                    String familyName = "";
                    while (!(eventType == XmlPullParser.END_TAG && parser.getName().equals("DriverStanding"))) {
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("GivenName")) {
                            givenName = parser.nextText();
                        }
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("FamilyName")) {
                            familyName = parser.nextText();
                        }
                        eventType = parser.next();
                    }
                    driverStandings.add(new String[]{position, givenName + " " + familyName, points, wins});
                }
                eventType = parser.next();
            }

            StringBuilder formattedData = new StringBuilder();
            formattedData.append("<html><body><table border='1' style='border-collapse: collapse;'>");
            formattedData.append("<tr><th>Posición</th><th>Piloto</th><th>Puntos</th><th>Victorias</th></tr>");
            for (String[] driver : driverStandings) {
                formattedData.append("<tr>");
                formattedData.append("<td>").append(driver[0]).append("</td>");
                formattedData.append("<td>").append(driver[1]).append("</td>");
                formattedData.append("<td>").append(driver[2]).append("</td>");
                formattedData.append("<td>").append(driver[3]).append("</td>");
                formattedData.append("</tr>");
            }
            formattedData.append("</table></body></html>");
            return formattedData.toString();
        }
    }

    private class FetchConstructorStandings extends AsyncTask<Void, Void, String> {

        @Override
        protected String doInBackground(Void... voids) {
            String apiUrl = "https://ergast.com/api/f1/current/constructorStandings";

            try {
                URL url = new URL(apiUrl);
                HttpURLConnection connection = (HttpURLConnection) url.openConnection();
                connection.setRequestMethod("GET");

                InputStream inputStream = connection.getInputStream();
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null) {
                    stringBuilder.append(line).append("\n");
                }
                bufferedReader.close();
                return stringBuilder.toString();
            } catch (IOException e) {
                Log.e("F1Activity", "Error al obtener datos de la API", e);
                return null;
            }
        }

        @Override
        protected void onPostExecute(String result) {
            if (result != null) {
                Log.d("F1Activity", "Respuesta de la API: " + result);
                try {
                    String parsedData = parseConstructorStandingsXML(result);
                    WebView apiResponseWebView = findViewById(R.id.constructorApiResponseWebView);
                    apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                    apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
                } catch (Exception e) {
                    Log.e("F1Activity", "Error al parsear los datos del constructor", e);
                }
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseConstructorStandingsXML(String xml) throws XmlPullParserException, IOException {
            XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
            XmlPullParser parser = factory.newPullParser();
            parser.setInput(new StringReader(xml));

            List<String[]> constructorStandings = new ArrayList<>();
            int eventType = parser.getEventType();
            while (eventType != XmlPullParser.END_DOCUMENT) {
                if (eventType == XmlPullParser.START_TAG && parser.getName().equals("ConstructorStanding")) {
                    String position = parser.getAttributeValue(null, "position");
                    String points = parser.getAttributeValue(null, "points");
                    String constructorName = "";
                    while (!(eventType == XmlPullParser.END_TAG && parser.getName().equals("ConstructorStanding"))) {
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Name")) {
                            constructorName = parser.nextText();
                        }
                        eventType = parser.next();
                    }
                    constructorStandings.add(new String[]{position, constructorName, points});
                }
                eventType = parser.next();
            }

            StringBuilder formattedData = new StringBuilder();
            formattedData.append("<html><body><table border='1' style='border-collapse: collapse;'>");
            formattedData.append("<tr><th>Posición</th><th>Constructor</th><th>Puntos</th></tr>");
            for (String[] constructor : constructorStandings) {
                formattedData.append("<tr>");
                formattedData.append("<td>").append(constructor[0]).append("</td>");
                formattedData.append("<td>").append(constructor[1]).append("</td>");
                formattedData.append("<td>").append(constructor[2]).append("</td>");
                formattedData.append("</tr>");
            }
            formattedData.append("</table></body></html>");
            return formattedData.toString();
        }
    }

    private class FetchSchedule extends AsyncTask<Void, Void, String> {

        @Override
        protected String doInBackground(Void... voids) {
            String apiUrl = "https://ergast.com/api/f1/current";

            try {
                URL url = new URL(apiUrl);
                HttpURLConnection connection = (HttpURLConnection) url.openConnection();
                connection.setRequestMethod("GET");

                InputStream inputStream = connection.getInputStream();
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null) {
                    stringBuilder.append(line).append("\n");
                }
                bufferedReader.close();
                return stringBuilder.toString();
            } catch (IOException e) {
                Log.e("F1Activity", "Error al obtener los datos de la API", e);
                return null;
            }
        }    @Override
        protected void onPostExecute(String result) {
            if (result != null) {
                Log.d("F1Activity", "Respuesta de la API: " + result);
                try {
                    String parsedData = parseScheduleXML(result);
                    WebView apiResponseWebView = findViewById(R.id.scheduleApiResponseWebView);
                    apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                    apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
                } catch (Exception e) {
                    Log.e("F1Activity", "Error al parsear los datos del horario de carreras", e);
                }
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseScheduleXML(String xml) throws XmlPullParserException, IOException {
            XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
            XmlPullParser parser = factory.newPullParser();
            parser.setInput(new StringReader(xml));

            List<String[]> scheduleList = new ArrayList<>();
            int eventType = parser.getEventType();
            while (eventType != XmlPullParser.END_DOCUMENT) {
                if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Race")) {
                    String raceName = "";
                    String date = "";
                    String time = "";
                    eventType = parser.next();
                    while (!(eventType == XmlPullParser.END_TAG && parser.getName().equals("Race"))) {
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("RaceName")) {
                            raceName = parser.nextText();
                        }
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Date")) {
                            date = parser.nextText();
                        }
                        if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Time")) {
                            time = parser.nextText();
                        }
                        eventType = parser.next();
                    }
                    String madridTime = convertUTCToMadridTime(date, time);
                    scheduleList.add(new String[]{raceName, date, madridTime});
                }
                eventType = parser.next();
            }

            StringBuilder formattedData = new StringBuilder();
            formattedData.append("<html><body><table border='1' style='border-collapse: collapse;'>");
            formattedData.append("<tr><th>Carrera</th><th>Fecha</th><th>Hora (Madrid)</th></tr>");
            for (String[] race : scheduleList) {
                formattedData.append("<tr>");
                formattedData.append("<td>").append(race[0]).append("</td>");
                formattedData.append("<td>").append(race[1]).append("</td>");
                formattedData.append("<td>").append(race[2]).append("</td>");
                formattedData.append("</tr>");
            }
            formattedData.append("</table></body></html>");
            return formattedData.toString();
        }

        private String convertUTCToMadridTime(String date, String time) {
            SimpleDateFormat utcFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");
            utcFormat.setTimeZone(TimeZone.getTimeZone("UTC"));

            SimpleDateFormat madridFormat = new SimpleDateFormat("HH:mm");
            madridFormat.setTimeZone(TimeZone.getTimeZone("Europe/Madrid"));

            try {
                Date utcDate = utcFormat.parse(date + "T" + time + "Z");
                return madridFormat.format(utcDate);
            } catch (ParseException e) {
                Log.e("F1Activity", "Error al convertir hora de UTC a Madrid", e);
                return time;
            }
        }
    }
}