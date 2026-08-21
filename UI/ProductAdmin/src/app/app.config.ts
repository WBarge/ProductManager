import { ApplicationConfig,importProvidersFrom } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import {provideHttpClient} from '@angular/common/http';
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes, withComponentInputBinding()),
              provideHttpClient(),
              providePrimeNG({
                  theme: {
                    preset: Aura,
                    // Default options,
                    options: {
                        prefix: 'p',
                        darkModeSelector: 'system',
                        cssLayer: false,
                        cssVariables: true
                    }
                  },
                license: 'eyJpZCI6IjgxYmZmNTc5LTBjOGItNDFmYS1iZWZlLTkzMTMxZDc1ZTExYiIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3ODcyNzMyNjQsImV4cCI6MTgxODgwOTI2NH0.uKb3JsOGna6TXt89RUwLsDpXohO5YuDjOkFE9WOhzpCL4CT6S3LIhcNaxT3qYs_6isvQ-AkjByTYfI5WkOQkCg'
              }),
              MessageService]
};
