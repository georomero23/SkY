import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EstadisticasDashboardComponent } from './estadisticas-dashboard.component';

describe('EstadisticasDashboardComponent', () => {
  let component: EstadisticasDashboardComponent;
  let fixture: ComponentFixture<EstadisticasDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EstadisticasDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EstadisticasDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
