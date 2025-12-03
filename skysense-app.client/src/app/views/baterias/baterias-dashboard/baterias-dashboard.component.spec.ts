import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BateriasDashboardComponent } from './baterias-dashboard.component';

describe('BateriasDashboardComponent', () => {
  let component: BateriasDashboardComponent;
  let fixture: ComponentFixture<BateriasDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BateriasDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BateriasDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
