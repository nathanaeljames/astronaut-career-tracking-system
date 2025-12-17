import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AstronautDutyService } from '../../../core/services/astronaut-duty.service';
import { PersonService } from '../../../core/services/person.service';

@Component({
  selector: 'app-duty-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './duty-history.html',
  styleUrl: './duty-history.scss'
})
export class DutyHistoryComponent {
  selectedPersonName = '';
  people: any[] = [];
  duties: any[] = [];
  person: any = null;

  loadingPeople = false;
  loadingDuties = false;
  error = '';

  constructor(
    private dutyService: AstronautDutyService,
    private personService: PersonService,
    private cdr: ChangeDetectorRef
  ) {
    this.loadPeople();
  }

  loadPeople() {
    this.loadingPeople = true;
    this.personService.getAllPeople().subscribe({
      next: (response) => {
        this.people = response.people || [];
        this.loadingPeople = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading people:', err);
        this.loadingPeople = false;
        this.cdr.detectChanges();
      }
    });
  }

  onPersonSelected() {
    if (!this.selectedPersonName) {
      this.duties = [];
      this.person = null;
      return;
    }

    this.loadingDuties = true;
    this.error = '';
    this.duties = [];
    this.person = null;

    this.dutyService.getAstronautDutiesByName(this.selectedPersonName).subscribe({
      next: (response) => {
        this.person = response.person;
        this.duties = response.astronautDuties || [];
        this.loadingDuties = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = err.message;
        this.loadingDuties = false;
        this.cdr.detectChanges();
      }
    });
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'Present';
    return new Date(dateString).toLocaleDateString();
  }
}