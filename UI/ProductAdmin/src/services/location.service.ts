import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocationService {

  private serviceUrl!: string;

  constructor() { }

  //these can be refactored into getter/setters look at the formattedName property of the person model as an example

  //set the location of the sevice
  public setLocationUrl(url: string) {
      this.serviceUrl = url;
  }

  //get the location of the service
  public getLocationUrl(): string {
      return this.serviceUrl;
  }
}
