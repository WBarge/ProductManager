import { Service,inject } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient  } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { OptionSelection } from '../models/results/optionSelection';
import { OptionListResult } from '../models/results/OptionListResult';


@Service()
export class OptionService {
  private http=inject(HttpClient);
  private location=inject(LocationService);
  private httpErrorHandler=inject( ErrorHandlerService);

  private optionsServiceLocation:string;
  private optionServiceLocation:string;
  private handleError: HandleError;

  constructor() {
    this.optionsServiceLocation = this.location.getLocationUrl()+'Options';
    this.optionServiceLocation = this.location.getLocationUrl()+'Option';
    this.handleError = this.httpErrorHandler.createHandleError('OptionService');
  }

  getOptions():Observable<OptionSelection[]>{
    let body = {};
    return this.http.post<OptionListResult>(this.optionsServiceLocation, body, { responseType: 'json' })
    .pipe(
      map((options: OptionListResult) => {
        return options.data.map(data => ({
          label: data.name,
          idValue: data.id,
          price: data.price
        }));
      }),
      catchError(this.handleError('getOptions', []))
    );
  }

}
