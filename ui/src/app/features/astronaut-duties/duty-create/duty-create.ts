import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AstronautDutyService } from '../../../core/services/astronaut-duty.service';
import { PersonService } from '../../../core/services/person.service';

@Component({
  selector: 'app-duty-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './duty-create.html',
  styleUrl: './duty-create.scss'
})
export class DutyCreateComponent {
  // Form fields
  selectedPersonName = '';
  rank = '';
  dutyTitle = '';
  dutyStartDate = '';

  // UI state
  people: any[] = [];
  loading = false;
  loadingPeople = false;
  error = '';
  success = '';

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

  onSubmit() {
    // Validation
    if (!this.selectedPersonName.trim()) {
      this.error = 'Please select a person';
      return;
    }
    if (!this.rank.trim()) {
      this.error = 'Please enter a rank';
      return;
    }
    if (!this.dutyTitle.trim()) {
      this.error = 'Please enter a duty title';
      return;
    }
    if (!this.dutyStartDate) {
      this.error = 'Please select a start date';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';

    const request = {
      name: this.selectedPersonName,
      rank: this.rank,
      dutyTitle: this.dutyTitle,
      dutyStartDate: this.dutyStartDate
    };

    this.dutyService.createAstronautDuty(request).subscribe({
      next: (response) => {
        this.success = `Duty assignment created successfully for ${this.selectedPersonName}!`;

        // Reset form
        this.selectedPersonName = '';
        this.rank = '';
        this.dutyTitle = '';
        this.dutyStartDate = '';

        this.loading = false;
        this.cdr.detectChanges();

        // Clear success message after 3 seconds
        setTimeout(() => {
          this.success = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (err) => {
        this.error = err.message;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}