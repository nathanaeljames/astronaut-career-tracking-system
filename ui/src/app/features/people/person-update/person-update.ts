import { Component, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonService } from '../../../core/services/person.service';

@Component({
  selector: 'app-person-update',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './person-update.html',
  styleUrl: './person-update.scss'
})
export class PersonUpdateComponent {
  @Output() personUpdated = new EventEmitter<void>();
  people: any[] = [];
  selectedPersonName = '';
  newName = '';

  loadingPeople = false;
  loading = false;
  error = '';
  success = '';

  constructor(
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
    // Pre-fill new name with current name
    if (this.selectedPersonName) {
      this.newName = this.selectedPersonName;
      this.error = '';
      this.success = '';
    } else {
      this.newName = '';
    }
  }

  onSubmit() {
    if (!this.selectedPersonName.trim()) {
      this.error = 'Please select a person to update';
      return;
    }
    if (!this.newName.trim()) {
      this.error = 'Please enter a new name';
      return;
    }
    if (this.newName === this.selectedPersonName) {
      this.error = 'New name must be different from current name';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';

    this.personService.updatePerson(this.selectedPersonName, this.newName).subscribe({
      next: (response) => {
        this.success = `Successfully updated "${this.selectedPersonName}" to "${this.newName}"`;
        console.log('Update API call successful!');

        // Reset form
        const oldName = this.selectedPersonName;
        this.selectedPersonName = '';
        this.newName = '';

        // Reload people list
        this.loadPeople();

        this.loading = false;
        console.log('About to emit personUpdated event...');
        this.personUpdated.emit();
        console.log('personUpdated event emitted!');
        this.cdr.detectChanges();

        // Clear success message after 5 seconds
        setTimeout(() => {
          this.success = '';
          this.cdr.detectChanges();
        }, 5000);
      },
      error: (err) => {
        console.error('Update API call failed:', err);
        this.error = err.message;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}