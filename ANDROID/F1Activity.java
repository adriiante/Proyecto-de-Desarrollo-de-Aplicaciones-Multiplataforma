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
                // Parsear la respuesta XML y mostrar los datos en el WebView
                String parsedData = parseDriverStandingsXML(result);
                WebView apiResponseWebView = findViewById(R.id.pilotsApiResponseWebView);
                apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseDriverStandingsXML(String xml) {
            try {
                XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
                XmlPullParser parser = factory.newPullParser();
                parser.setInput(new StringReader(xml));
                List<String> driverStandings = new ArrayList<>();
                int eventType = parser.getEventType();
                while (eventType != XmlPullParser.END_DOCUMENT) {
                    if (eventType == XmlPullParser.START_TAG && parser.getName().equals("DriverStanding")) {
                        String driverInfo = "";
                        String position = parser.getAttributeValue(null, "position");
                        String points = parser.getAttributeValue(null, "points"); // Obtener los puntos
                        String wins = parser.getAttributeValue(null, "wins"); // Obtener las victorias
                        eventType = parser.next();
                        while (!(eventType == XmlPullParser.END_TAG && parser.getName().equals("DriverStanding"))) {
                            if (eventType == XmlPullParser.START_TAG && parser.getName().equals("GivenName")) {
                                driverInfo += parser.nextText() + " ";
                            }
                            if (eventType == XmlPullParser.START_TAG && parser.getName().equals("FamilyName")) {
                                driverInfo += parser.nextText();
                            }
                            eventType = parser.next();
                        }
                        driverInfo += " - " + points + " puntos - " + wins + " victorias esta temporada";
                        String formattedInfo = String.format("%-3s", position) + "&nbsp;&nbsp;&nbsp;&nbsp;" + driverInfo;
                        driverStandings.add(formattedInfo);
                    }
                    eventType = parser.next();
                }
                StringBuilder formattedData = new StringBuilder();
                formattedData.append("<html><body>");
                for (String driver : driverStandings) {
                    formattedData.append("<div style='margin-left:10px; margin-bottom: -12px'>").append(driver).append("</div><br>");
                }
                formattedData.append("</body></html>");
                return formattedData.toString();
            } catch (XmlPullParserException | IOException e) {
                Log.e("F1Activity", "Error al parsear XML", e);
                return "Error al parsear XML";
            }
        }
    }

    private class FetchConstructorStandings extends AsyncTask<Void, Void, String> {

        @Override
        protected String doInBackground(Void... voids) {
            String apiUrl = "https://ergast.com/api/f1/current/constructorstandings";

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
                // Parsear la respuesta XML y mostrar los datos en el WebView
                String parsedData = parseConstructorStandingsXML(result);
                WebView apiResponseWebView = findViewById(R.id.constructorsApiResponseWebView);
                apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseConstructorStandingsXML(String xml) {
            try {
                XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
                XmlPullParser parser = factory.newPullParser();
                parser.setInput(new StringReader(xml));

                List<String> constructorStandings = new ArrayList<>();
                int eventType = parser.getEventType();
                while (eventType != XmlPullParser.END_DOCUMENT) {
                    if (eventType == XmlPullParser.START_TAG && parser.getName().equals("ConstructorStanding")) {
                        String constructorName = "";
                        String points = parser.getAttributeValue(null, "points"); // Obtener directamente los puntos del atributo
                        String position = parser.getAttributeValue(null, "position");
                        eventType = parser.next();
                        while (!(eventType == XmlPullParser.END_TAG && parser.getName().equals("ConstructorStanding"))) {
                            if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Name")) {
                                constructorName = parser.nextText();
                            }
                            eventType = parser.next();
                        }
                        String constructorInfo = String.format("%-3s", position)
                                + "&nbsp;&nbsp;&nbsp;&nbsp;"
                                + constructorName
                                + " - " + points; // Añadir los puntos
                        constructorStandings.add(constructorInfo);
                    }
                    eventType = parser.next();
                }

                StringBuilder formattedData = new StringBuilder();
                formattedData.append("<html><body>");
                for (String constructor : constructorStandings) {
                    formattedData.append("<div style='margin-left:10px; margin-bottom: -12px'>").append(constructor).append("</div><br>");
                }
                formattedData.append("</body></html>");
                return formattedData.toString();
            } catch (XmlPullParserException | IOException e) {
                Log.e("F1Activity", "Error al parsear XML", e);
                return "Error al parsear XML";
            }
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
                Log.e("F1Activity", "Error al obtener datos de la API", e);
                return null;
            }
        }

        @Override
        protected void onPostExecute(String result) {
            if (result != null) {
                Log.d("F1Activity", "Respuesta de la API: " + result);
                // Parsear la respuesta XML y mostrar los datos en el WebView
                String parsedData = parseScheduleXML(result);
                WebView apiResponseWebView = findViewById(R.id.scheduleApiResponseWebView);
                apiResponseWebView.getSettings().setJavaScriptEnabled(true);
                apiResponseWebView.loadData(parsedData, "text/html; charset=utf-8", "UTF-8");
            } else {
                Log.e("F1Activity", "La respuesta de la API es nula");
            }
        }

        private String parseScheduleXML(String xml) {
            // Método para parsear el XML del horario de carreras

            try {
                XmlPullParserFactory factory = XmlPullParserFactory.newInstance();
                XmlPullParser parser = factory.newPullParser();
                parser.setInput(new StringReader(xml));

                List<String> scheduleList = new ArrayList<>();
                int eventType = parser.getEventType();
                while (eventType != XmlPullParser.END_DOCUMENT) {
                    if (eventType == XmlPullParser.START_TAG && parser.getName().equals("Race")) {
                        String raceInfo = "";
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
                        // Convertir la hora UTC a la hora de Madrid
                        String madridTime = convertUTCToMadridTime(date, time);
                        raceInfo = raceName + " - El día " + date.substring(5) + " - " + madridTime;
                        scheduleList.add(raceInfo);
                    }
                    eventType = parser.next();
                }

                StringBuilder formattedData = new StringBuilder();
                formattedData.append("<html><body>");
                for (String race : scheduleList) {
                    formattedData.append("<div style='margin-left:10px; margin-bottom: -12px'>").append(race).append("</div><br>");
                }
                formattedData.append("</body></html>");
                return formattedData.toString();
            } catch (XmlPullParserException | IOException e) {
                Log.e("F1Activity", "Error al parsear el XML del horario de carreras", e);
                return "Error al parsear el XML del horario de carreras";
            }
        }



        private String convertUTCToMadridTime(String date, String time) {
            try {
                SimpleDateFormat inputDateFormat = new SimpleDateFormat("yyyy-MM-dd");
                SimpleDateFormat outputDateFormat = new SimpleDateFormat("dd/MM");

                SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
                sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
                Date utcDate = sdf.parse(date + "T" + time);

                sdf.setTimeZone(TimeZone.getTimeZone("Europe/Madrid"));
                String formattedDate = outputDateFormat.format(utcDate);
                String formattedTime = new SimpleDateFormat("HH:mm").format(utcDate);
                return formattedDate.substring(5) + " a las " + formattedTime;
            } catch (ParseException e) {
                Log.e("F1Activity", "Error al convertir la hora UTC a la hora de Madrid", e);
                return "Error al convertir la hora UTC a la hora de Madrid";
            }
        }


    }
}
