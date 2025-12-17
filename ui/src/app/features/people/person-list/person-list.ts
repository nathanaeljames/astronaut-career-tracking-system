import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PersonService } from '../../../core/services/person.service';
import { PersonCreateComponent } from '../person-create/person-create'; 
import { PersonUpdateComponent } from '../person-update/person-update';

@Component({
  selector: 'app-person-list',
  standalone: true,
  imports: [CommonModule, PersonCreateComponent, PersonUpdateComponent],
  templateUrl: './person-list.html',
  styleUrl: './person-list.scss'
})
export class PersonListComponent implements OnInit {
  people: any[] = [];
  loading = true;
  error = '';

  constructor(
    private personService: PersonService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadPeople();
  }

  loadPeople() {
    this.loading = true;
    console.log('Starting to load people...');
    this.personService.getAllPeople().subscribe({
      next: (response) => {
        console.log('Received response:', response);
        console.log('People array:', response.people);
        this.people = response.people || [];
        this.loading = false;
        this.cdr.detectChanges();
        console.log('Loading complete. People count:', this.people.length);
      },
      error: (err) => {
        console.error('Error loading people:', err);
        this.error = err.message;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPersonUpdated() {
    console.log('PersonListComponent: Received personUpdated event!');
    this.loadPeople();
  }
}