import { Component, OnInit } from "@angular/core";
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Bruker } from "../Bruker";
import { ReModal } from "./registereModal"

@Component({
  templateUrl: "loggin.html"
})

export class Loggin {
  skjema: FormGroup;
  errMsg: string;
  validering = {
    brukernavn: [
      null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-.1-9 ]{2,30}")])
    ],
    passord: [
      null, Validators.compose([Validators.required])
    ]
  }
  constructor(private http: HttpClient, private fb: FormBuilder, private router: Router, private modalService: NgbModal) {
    this.skjema = fb.group(this.validering);
  }

  vedSubmit() {
      this.loggin();
  }

  loggin() {
    const b = new Bruker();

    b.brukernavn = this.skjema.value.brukernavn;
    
    b.password = this.skjema.value.passord;
    console.log(b)
   

    this.http.post("api/stock/loggin", b)
      .subscribe(retur => {
        console.log(retur);
        this.router.navigate(['/home']);
      },

        error => { console.log(error), this.errMsg = "Feil brukernavn eller passord"  }
        
      );
  };
  Registere() {
    const modalRef = this.modalService.open(ReModal);
    modalRef.result.then(retur => {
      console.log('Lukket med:' + retur);
      if (retur == "bekreft") {
        this.router.navigate(['/loggin']);
      }
      this.router.navigate(['/loggin']);
    });
   
  }
}
