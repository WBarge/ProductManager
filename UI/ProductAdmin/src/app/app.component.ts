import { Component,OnInit,ElementRef } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api'
import { MenuItem } from 'primeng/api';
import { MenubarModule} from 'primeng/menubar';
import { LocationService } from '../services/location.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,MenubarModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public items: MenuItem[];
  public title: any = "Product Admin";

  constructor (private primengConfig: PrimeNGConfig,private locationService:LocationService,private elementRef: ElementRef,){
    this.items = [
       { label: 'Products', routerLink: ['/products'] },
       { label: 'About', routerLink: ['/about'] }
      ];
  }

  ngOnInit(): void {
      this.primengConfig.ripple = true;
      this.locationService.setLocationUrl(this.elementRef.nativeElement.getAttribute('serviceUrl'));
  }
}
