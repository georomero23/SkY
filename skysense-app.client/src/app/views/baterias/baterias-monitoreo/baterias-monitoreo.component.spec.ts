import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BateriasMonitoreoComponent } from './baterias-monitoreo.component';

describe('BateriasMonitoreoComponent', () => {
  let component: BateriasMonitoreoComponent;
  let fixture: ComponentFixture<BateriasMonitoreoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BateriasMonitoreoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BateriasMonitoreoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
