import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiasInhabilesComponent } from './dias-inhabiles.component';

describe('DiasInhabilesComponent', () => {
  let component: DiasInhabilesComponent;
  let fixture: ComponentFixture<DiasInhabilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DiasInhabilesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiasInhabilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
