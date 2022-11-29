import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-meny',
  templateUrl: './meny.html'
})
export class Meny {
  isExpanded = false;
  
  constructor(private http: HttpClient, private router: Router) { }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
  loggut() {
    this.http.get("api/stock/LoggUt")
      .subscribe(retur => {
        console.log(retur);
        this.router.navigate(['/loggin']);
      },
        error => console.log(error)
      );
  }
}
