import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Component, OnInit, } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { BrukerStocks } from "../BrukerStocks";


@Component({
  templateUrl: 'selgeModal.html'
})

export class SelgeModal {
  skjemaS: FormGroup;
  
  validering = {
     SId: [
      null, Validators.compose([Validators.required, Validators.min(1)])
    ],
    BSId: [
      null, Validators.compose([Validators.required, Validators.min(1)])
    ],
    antallStocks: [
      null, Validators.compose([Validators.required, Validators.min(1), Validators.pattern(/^[0-9]\d*$/)])
    ]
   
  }

  constructor(private http: HttpClient, private router: Router, private fb: FormBuilder, public modal: NgbActiveModal) {
    this.skjemaS = fb.group(this.validering);
  }

  Selge() {
    const bs = new BrukerStocks();
    bs.antallStock = this.skjemaS.value.antallStocks;
    bs.SId = this.skjemaS.value.SId;
    bs.BSId = this.skjemaS.value.BSId;
    console.log(bs)

    this.http.put("api/stock/slegeStocks", bs)
      .subscribe(retur => {
        this.modal.close("bekreft");
      },
        error => console.log(error)
    );
    
    
  }
}

