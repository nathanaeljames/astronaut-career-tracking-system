import { Component, EventEmitter, Output, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonService } from '../../../core/services/person.service';

@Component({
  selector: 'app-person-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './person-create.html',
  styleUrl: './person-create.scss'
})
export class PersonCreateComponent {
  @Output() personCreated = new EventEmitter<void>();

  personName = '';
  loading = false;
  error = '';
  success = '';

  constructor(
    private personService: PersonService,
    private cdr: ChangeDetectorRef
  ) { }

  onSubmit() {
    if (!this.personName.trim()) {
      this.error = 'Please enter a name';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';

    this.personService.createPerson(this.personName).subscribe({
      next: (response) => {
        this.success = `Person "${this.personName}" created successfully! ID: ${response.id}`;
        this.personName = '';
        this.loading = false;
        this.personCreated.emit();
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