import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BateriasConsultaComponent } from './baterias-consulta.component';

describe('BateriasConsultaComponent', () => {
  let component: BateriasConsultaComponent;
  let fixture: ComponentFixture<BateriasConsultaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BateriasConsultaComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BateriasConsultaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
