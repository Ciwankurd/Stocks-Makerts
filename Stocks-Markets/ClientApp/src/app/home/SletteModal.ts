import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: 'SletteModal.html'
})
export class SletteModal {
  constructor(public modal: NgbActiveModal) { }
}
