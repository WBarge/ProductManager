import { ApplicationConfig,importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { routes } from './app.routes';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
              provideAnimations(),
              importProvidersFrom(HttpClientModule),
              MessageService]
};
