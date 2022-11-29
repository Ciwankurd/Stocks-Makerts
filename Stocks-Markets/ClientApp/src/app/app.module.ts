import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Home } from './home/home';
import { SletteModal } from './home/SletteModal';
import { AppComponent } from './app.component';
import { Lagre } from './lagre/lagre';
import { Endre } from './endre/endre';
import { Meny } from './meny/meny';
import { Loggin } from './loggin/loggin';
import { AppRoutingModule } from './app-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { KjopeModal } from './home/KjopeModal';
import { ReModal } from './loggin/registereModal';
import { MinStocks } from './MinStocks/minStocks';
import { SelgeModal } from './MinStocks/selgeModal';

@NgModule({
  declarations: [
    AppComponent,
    Lagre,
    Endre,
    Meny,
    Loggin,
    ReModal,
    Home,
    SletteModal,
    KjopeModal,
    MinStocks,
    SelgeModal
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [ReModal, SletteModal, KjopeModal, SelgeModal] 
})
export class AppModule { }
