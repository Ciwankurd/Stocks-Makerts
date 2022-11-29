import { Component, OnInit } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Stock } from "../Stock";

@Component({
  templateUrl: "endre.html"
})
export class Endre {
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

  constructor(private http: HttpClient, private fb: FormBuilder,
              private route: ActivatedRoute, private router: Router) {
      this.skjema = fb.group(this.validering);
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.hentEnStock(params.id);
    })
  }

  vedSubmit() {
      this.endreStock();
  }

  hentEnStock(Sid: number) {
    this.http.get<Stock>("api/stock/hentEnStock" + Sid)
      .subscribe(
        stock => {
          console.log(stock)
          this.skjema.patchValue({ SId: stock.sId });
          this.skjema.patchValue({ tegn: stock.tegn });
          this.skjema.patchValue({ selskap: stock.selskapNavn });
          this.skjema.patchValue({ antallstock: stock.antallStock });
          this.skjema.patchValue({ endring: stock.endring });
          this.skjema.patchValue({ sistepris: stock.sistePrise });
          this.skjema.patchValue({ volume: stock.volume });
        },
        error => console.log(error)
      );
  }

  endreStock() {
    const endretStock = new Stock();
    endretStock.sId = this.skjema.value.SId;
    endretStock.tegn = this.skjema.value.tegn;
    endretStock.selskapNavn = this.skjema.value.selskap;
    endretStock.antallStock = this.skjema.value.antallstock;
    endretStock.endring = this.skjema.value.endring;
    endretStock.sistePrise = this.skjema.value.sistepris;
    endretStock.volume = this.skjema.value.volume;

    this.http.put("api/stock/endreStock", endretStock)
      .subscribe(retur => {
        console.log(retur);
        this.router.navigate(['/home']);
        },
        error => console.log(error)
    );
  }
}
