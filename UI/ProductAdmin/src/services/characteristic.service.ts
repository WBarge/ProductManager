import { Service,inject } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Characteristic } from '../models/results/characteristic';
import { CharacteristicValue } from '../models/results/characteristic-value';

@Service()
export class CharacteristicService {
  private http=inject(HttpClient);
  private location=inject(LocationService);
  private httpErrorHandler=inject( ErrorHandlerService);

  private characteristicsServiceLocation:string;
  private characteristicServiceLocation:string;
  private handleError: HandleError;

  constructor(){
    this.characteristicsServiceLocation = this.location.getLocationUrl()+'Characteristics';
    this.characteristicServiceLocation = this.location.getLocationUrl()+'Characteristic';
    this.handleError = this.httpErrorHandler.createHandleError('CharacteristicService');
  }

  getCharacteristics():Observable<Characteristic[]>{
    const requstUrl = this.characteristicsServiceLocation;
    return this.http.get<Characteristic[]>(requstUrl)
      .pipe(
        map((characts:any[])=>{
          return characts.map((charact:any)=>{
            const returnValue = new Characteristic();
            returnValue.idValue = charact.id;
            returnValue.name = charact.name;
            returnValue.values = (charact.values ?? []).map((cv:any)=>{
              const charVal = new CharacteristicValue();
              charVal.idValue = cv.id;
              charVal.value = cv.value;
              return charVal;
            });
            return returnValue;
          });
        }),
        catchError(this.handleError<Characteristic[]>('getCharacteristics', []))
      );
  }

  getCharacteristic(characteristicId:string):Observable<Characteristic>{
    const requstUrl = this.characteristicsServiceLocation+'/'+characteristicId;
    return this.http.get<Characteristic>(requstUrl)
      .pipe(
        map((charact:any)=>{
          const returnValue = new Characteristic();
          returnValue.idValue = charact.id;
          returnValue.name = charact.name;
            returnValue.values = (charact.values ?? []).map((cv:any)=>{
            const charVal = new CharacteristicValue();
            charVal.idValue = cv.id;
            charVal.value = cv.value;
            return charVal;
          });
          return returnValue;
        }),
        catchError(this.handleError<Characteristic>('getCharacteristic'))
      );
  }

  deleteCharacteristic(characteristicId:string):Observable<any>{
    const requstUrl = this.characteristicServiceLocation+'/'+characteristicId;
    return this.http.delete(requstUrl).pipe(catchError(this.handleError<any>('deleteCharacteristic')));
  }

  addCharacteristic(characteristic:string):Observable<string>{
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<{ id: string }>(
      this.characteristicServiceLocation,
      JSON.stringify(characteristic),
      { headers, responseType: 'json' }
    )
      .pipe(
        map((result) =>{
          return result.id;
        }),
        catchError(this.handleError<string>('addCharacteristic'))
      );
  }

  addCharacteristicValue(characteristicId:string,valueToAdd:string):Observable<string>{
    const requstUrl = this.characteristicServiceLocation+'/'+characteristicId+'/value';
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post<{ id: string }>(requstUrl,JSON.stringify(valueToAdd),{ headers })
     .pipe(
        map((data) => data.id),
        catchError(this.handleError<string>('addCharacteristicValue'))
      );
  }

  deleteCharacteristicValue(characteristicId:string,characteristicValueId:string):Observable<any>{
    const requstUrl = this.characteristicServiceLocation+'/'+characteristicId+'/value/'+characteristicValueId;
    return this.http.delete(requstUrl).pipe(catchError(this.handleError<any>('deleteCharacteristicValue')));

  }


}
