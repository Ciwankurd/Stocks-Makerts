import { Component, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SletteModal } from './SletteModal';
import { KjopeModal } from './KjopeModal';
import { Stock } from "../Stock";
import { Bruker } from "../Bruker";

@Component({
  templateUrl: "home.html"
})
export class Home implements OnInit, OnChanges {
  AlleStocks: Array<Stock>;
  SlettStock: string;
  slettingOK: boolean;
  BrukerType: boolean;  // sjekk om det er Admin eller vanlig Bruker som logget inn
  logginOK: boolean;
  brukernavn: string;
  laster: boolean;
  constructor(private http: HttpClient, private router: Router, private modalService: NgbModal) { }
  ngOnChanges(changes: SimpleChanges): void {
    this.hentAlleStocks();
  }
  
  ngOnInit() {
    this.laster = true;
    this.hentEnbruker();
    this.hentAlleStocks();
  }
  
  hentAlleStocks() {
    
    this.http.get<Stock[]>("api/stock/hentAlleStocks")
      .subscribe(stocks => {
        
        this.AlleStocks = stocks;
        console.log(this.AlleStocks)
        this.laster = false;
      },
        error => console.log(error)
      );
  };

  hentEnbruker() {

    this.http.get<Bruker>("api/stock/hentEnBruker")
      .subscribe(Bruker => {
        console.log(Bruker)
        this.brukernavn = Bruker.brukernavn;
        this.logginOK = true;
        this.BrukerType = Bruker.bType;

      },
        error => console.log(error)
      );

  };
  
  sletteStock(SId: number) {
      const modalRef = this.modalService.open(SletteModal);

      modalRef.result.then(retur => {
        console.log('Lukket med:' + retur);
        if (retur == "Slett") {
          this.http.delete("api/stock/slettStock" + SId)
            .subscribe(retur => {
              this.hentAlleStocks();
              this.router.navigate(['/home']);
            },
              error => console.log(error)
          );
          this.router.navigate(['/home']);
        }
        
      });

    }
  kjopeStocks(SId: number) {
    
    const modalRef = this.modalService.open(KjopeModal);
    modalRef.componentInstance.SId = SId;
    modalRef.result.then(retur => {
      console.log('Lukket med:' + retur);
      if (retur == "bekreft") {
        this.router.navigate(['/minstock']);
      }
      this.router.navigate(['/minstock']);
    });
    
  }
}

