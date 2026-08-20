import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  IonButton,
  IonChip,
  IonContent,
  IonFabButton,
  IonHeader,
  IonIcon,
  IonLabel,
  IonSelect,
  IonSelectOption,
  IonTitle,
  IonToolbar,
} from '@ionic/angular';
import { HomePage } from './home.page';

import { HomePageRoutingModule } from './home-routing.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonButton,
    IonChip,
    IonContent,
    IonFabButton,
    IonHeader,
    IonIcon,
    IonLabel,
    IonSelect,
    IonSelectOption,
    IonTitle,
    IonToolbar,
    HomePageRoutingModule
  ],
  declarations: [HomePage]
})
export class HomePageModule {}
