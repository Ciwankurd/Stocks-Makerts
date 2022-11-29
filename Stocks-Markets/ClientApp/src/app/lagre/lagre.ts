import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Stock } from "../Stock";
@Component({
  templateUrl: "lagre.html"
})
export class Lagre {
  skjema: FormGroup;
  numRegex = /^-?\d*[.,]?\d{0,2}$/;
  validering = {
    SId: [""],
    tegn: [
      null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,6}")])
    ],
    selskap: [
      null, Validators.compose([Validators.required, Validators.pattern("[a-zA-ZøæåØÆÅ\\-. ]{2,30}")])
    ],
    antallstock: [
      null, Validators.compose([Validators.required, Validators.pattern("^[0-9]*$")])
    ],
    endring: [
      null, Validators.compose([Validators.required, Validators.pattern(this.numRegex)])
    ],
    sistepris: [
      null, Validators.compose([Validators.required, Validators.pattern(this.numRegex)])
    ],
    volume: [
      null, Validators.compose([Validators.required, Validators.pattern(this.numRegex)])
    ]
  }

  constructor(private http: HttpClient, private fb: FormBuilder, private router: Router) {
    this.skjema = fb.group(this.validering);
  }

  vedSubmit() {
      this.lagreKunde();
  }

  lagreKunde() {
    const lagreNyStock = new Stock();
    lagreNyStock.tegn = this.skjema.value.tegn;
    lagreNyStock.selskapNavn = this.skjema.value.selskap;
    lagreNyStock.antallStock = this.skjema.value.antallstock;
    lagreNyStock.endring = this.skjema.value.endring;
    lagreNyStock.sistePrise = this.skjema.value.sistepris;
    lagreNyStock.volume = this.skjema.value.volume;
    console.log(lagreNyStock)
    this.http.post("api/stock/lagreStock", lagreNyStock)
      .subscribe(retur => {
        console.log(retur);
        this.router.navigate(['/home']);
        },
        error => console.log(error)
      );
    
  }
}
