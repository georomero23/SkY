import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoConexionesFormComponent } from './info-conexiones-form.component';

describe('InfoConexionesFormComponent', () => {
  let component: InfoConexionesFormComponent;
  let fixture: ComponentFixture<InfoConexionesFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InfoConexionesFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InfoConexionesFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
