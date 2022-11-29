import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { BrukerStocks } from "../BrukerStocks";


@Component({
  templateUrl: 'KjopeModal.html'
})

export class KjopeModal {
  
  skjemak: FormGroup;
  
  validering = {
     SId: [
      null, Validators.compose([Validators.required, Validators.min(1)])
    ],
    antallStocks: [
      null, Validators.compose([Validators.required, Validators.min(1), Validators.pattern(/^[0-9]\d*$/)])
    ]
   
  }

  constructor(private http: HttpClient, private router: Router, private fb: FormBuilder, public modal: NgbActiveModal) {
    this.skjemak = fb.group(this.validering);
  }

  Kjope() {
    const bs = new BrukerStocks();
    bs.antallStock = this.skjemak.value.antallStocks;
    bs.SId = this.skjemak.value.SId;
    console.log(bs)

    this.http.post("api/stock/kjopestocks", bs)
      .subscribe(retur => {
        this.modal.close("bekreft")
        
      },
        error => console.log(error)
    );
    
  }
}

