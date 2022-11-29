import { Component, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SelgeModal } from './selgeModal';
import { Bruker } from "../Bruker";
import { BrukerStocks } from "../BrukerStocks";

@Component({
  templateUrl: "minStocks.html"
})
export class MinStocks implements OnInit, OnChanges {
  AlleBrukerStocks: Array<BrukerStocks>;
  logginOK: boolean;
  brukernavn: string;

  constructor(private http: HttpClient, private router: Router, private modalService: NgbModal) { }
  
  ngOnChanges(changes: SimpleChanges) {
    this.HentBrukerStocks();
    }
    
  ngOnInit() {
      
      this.hentEnbruker();
      this.HentBrukerStocks();
      
    }
  
  
 
  
  HentBrukerStocks() {  
    this.http.get<BrukerStocks[]>("api/stock/HentBrukerStocks")
      .subscribe(bs => {
        this.AlleBrukerStocks = bs;
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
      },
        error => console.log(error)
      );
  };
  

  slegeStocks(SId: number,BSId:number) {
    
    const modalRef = this.modalService.open(SelgeModal);
    modalRef.componentInstance.SId = SId;
    modalRef.componentInstance.BSId = BSId;
    modalRef.result.then(retur => {
      console.log('Lukket med:' + retur);
      if (retur == "bekreft") {
        this.HentBrukerStocks();
        this.router.navigate(['/minstock']);
      }
      this.router.navigate(['/minstock']);
    });
    
  }
}

