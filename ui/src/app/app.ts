import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavigationComponent } from './shared/components/navigation/navigation';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavigationComponent],
  template: `
    <div class="app-container">
      <header class="bg-primary text-white py-3">
        <div class="container-fluid">
          <h1 class="mb-0">Stargate ACTS - Astronaut Career Tracking System</h1>
        </div>
      </header>
      
      <app-navigation></app-navigation>
      
      <main class="container-fluid">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      min-height: 100vh;
      background-color: #f5f5f5;
    }

    header {
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);

      h1 {
        font-size: 1.75rem;
        margin: 0;
      }
    }

    main {
      padding: 20px;
    }
  `]
})
export class AppComponent {
  title = 'Stargate ACTS';
}