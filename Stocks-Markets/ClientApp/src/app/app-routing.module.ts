import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Lagre} from './lagre/lagre';
import { Endre } from './endre/endre';
import { Home } from './home/home';
import { MinStocks } from './MinStocks/minStocks';
import { Loggin } from './loggin/loggin';

const appRoots: Routes = [
  { path: 'loggin', component: Loggin },
  { path: 'lagre', component: Lagre },
  { path: 'endre/:id', component: Endre },
  { path: 'home', component: Home },
  { path: 'minstock', component: MinStocks },
  { path: '', redirectTo: 'loggin', pathMatch: 'full' }
]

@NgModule({
  imports: [
    RouterModule.forRoot(appRoots)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
