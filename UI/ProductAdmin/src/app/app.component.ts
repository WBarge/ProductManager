import { Component, OnInit, ElementRef, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { MenubarModule} from 'primeng/menubar';
import { LocationService } from '../services/location.service';
import { ToastModule } from 'primeng/toast';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, MenubarModule, ToastModule],
    templateUrl: './app.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public items: MenuItem[];
  public title: any = "Product Admin";

  constructor (private locationService:LocationService,private elementRef: ElementRef,){
    this.items = [
       { label: 'Products', routerLink: ['/products'] },
       { label: 'About', routerLink: ['/about'] }
      ];
  }

  ngOnInit(): void {
//      this.primengConfig.ripple = true;
      this.locationService.setLocationUrl(this.elementRef.nativeElement.getAttribute('serviceUrl'));
  }
}
