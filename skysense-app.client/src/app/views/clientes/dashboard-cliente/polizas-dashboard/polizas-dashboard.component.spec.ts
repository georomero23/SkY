import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PolizasDashboardComponent } from './polizas-dashboard.component';

describe('PolizasDashboardComponent', () => {
  let component: PolizasDashboardComponent;
  let fixture: ComponentFixture<PolizasDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PolizasDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PolizasDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
