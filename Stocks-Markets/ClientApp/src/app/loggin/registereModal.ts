import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Bruker } from "../Bruker";

@Component({
  templateUrl: 'registereModal.html'
})
export class ReModal {

  skjemaM: FormGroup;
  
  validering = {
    brukernavnM: [
      null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
    ],
    passordM: [
      null, Validators.compose([Validators.required, Validators.pattern("(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}")])
    ],

  }

  constructor(private http: HttpClient, private router: Router, private fb: FormBuilder, public modal: NgbActiveModal) {
    this.skjemaM = fb.group(this.validering);
  }

  Registere() {
    const b = new Bruker();
    b.brukernavn = this.skjemaM.value.brukernavnM;
    b.password = this.skjemaM.value.passordM;
    console.log(b)
        // kall til server for sletting
    this.http.post("api/stock/registereBruker", b)
      .subscribe(retur => {
      },
        error => console.log(error)
        );
    this.modal.close("bekreft")
  }
}

