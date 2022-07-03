import { Component } from '@angular/core';

import * as signalR from '@microsoft/signalr'
import * as Highcharts from "highcharts";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  connection : signalR.HubConnection;

constructor() {
  this.connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:44361/satishub")
  .build();

  this.connection.start();

  this.connection.on("receiveMessage",message => {
    alert(message);
  });
}

  title = 'ChartsClient';
  Highcharts : typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {
    // Grafik Title
    title:{
      text: "Başlık"
    },
    // Alt Title
    subtitle:{
      text: "Alt başlık"
    },
    // Y Ekseni
    yAxis:{
      title:{
        text:"Y Ekseni"
      }
    },
    //X Ekseni
    xAxis:{
      accessibility:{
        rangeDescription: "2022 - 2023"
      }
    },
    legend:{
      layout:"vertical",
      align:"right",
      verticalAlign:"middle"
    },
    series:[
      {
        name:"X",
        type:'line',
        data:[1000,2000,3000]
      },

      {
        name:"Y",
        type:'line',
        data:[1467,2240,980]
      },

      {
        name:"Z",
        type:'line',
        data:[3125,2152,3000]
      }
    ],
    plotOptions:{
      series:{
        label:{
          connectorAllowed:true
        },
        pointStart:100
      }
    }
  }
}
