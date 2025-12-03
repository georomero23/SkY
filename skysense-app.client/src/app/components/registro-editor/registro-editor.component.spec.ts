import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistroEditorComponent } from './registro-editor.component';

describe('RegistroEditorComponent', () => {
  let component: RegistroEditorComponent;
  let fixture: ComponentFixture<RegistroEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistroEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegistroEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
