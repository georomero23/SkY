import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PanelesComponent } from './paneles.component';

describe('PanelesComponent', () => {
  let component: PanelesComponent;
  let fixture: ComponentFixture<PanelesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PanelesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PanelesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
