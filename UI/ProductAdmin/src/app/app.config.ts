import { ApplicationConfig } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import {provideHttpClient} from '@angular/common/http';
import { routes } from './app.routes';
import { providePrimeNG } from 'primeng/config';
import {definePreset} from '@primeuix/themes';
import Lara from '@primeuix/themes/lara';
import { MessageService } from 'primeng/api';

const MyPreset = definePreset(Lara, {
  semantic: {
    primary: {
      50: '{purple.50}',
      100: '{purple.100}',
      200: '{purple.200}',
      300: '{purple.300}',
      400: '{purple.400}',
      500: '{purple.500}',
      600: '{purple.600}',
      700: '{purple.700}',
      800: '{purple.800}',
      900: '{purple.900}',
      950: '{purple.950}'
    }
  }
});

export const appConfig: ApplicationConfig = {

  providers: [provideRouter(routes, withComponentInputBinding()),
              providePrimeNG({
                  theme: {
                    preset: MyPreset,
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
              provideHttpClient(),
              MessageService]
};
