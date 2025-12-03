import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-multi-select',
  imports: [CommonModule, FormsModule],
  templateUrl: './multi-select.component.html',
  styleUrls: ['./multi-select.component.scss']
})
export class MultiSelectComponent {
  cerrando() {
    this.open = false;
    console.log("adusfee")
  }
  @Input() options: { value: string, label: string }[] = [];
  @Input() selected: string[] = [];
  @Output() selectedChange = new EventEmitter<string[]>();
  @Input() placeholder: string = 'Selecciona...';

  open = false;

  toggleDropdown() {
    this.open = !this.open;
  }

  toggleSelection(value: string, checked: any) {
    const newSelected = checked.checked
      ? [...this.selected, value]
      : this.selected.filter(v => v !== value);
    this.selectedChange.emit(newSelected);
  }

  getSelectedLabels(): string {
    return this.options
      .filter(opt => this.selected.includes(opt.value))
      .map(opt => opt.label)
      .join(', ');
  }
}
