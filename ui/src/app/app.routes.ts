import { Routes } from '@angular/router';
import { PersonListComponent } from './features/people/person-list/person-list';
import { DutyCreateComponent } from './features/astronaut-duties/duty-create/duty-create';
import { DutyHistoryComponent } from './features/astronaut-duties/duty-history/duty-history';

export const routes: Routes = [
    { path: '', redirectTo: '/people', pathMatch: 'full' },
    { path: 'people', component: PersonListComponent },
    { path: 'duties', component: DutyCreateComponent },
    { path: 'history', component: DutyHistoryComponent },
    { path: '**', redirectTo: '/people' }
];